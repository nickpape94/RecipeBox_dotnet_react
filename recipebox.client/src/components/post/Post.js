import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import { getPost } from '../../actions/post';
import AwesomeSlider from 'react-awesome-slider';
import AwsSliderStyles from 'react-awesome-slider/dist/styles.css';

const Post = ({ getPost, post: { post, loading }, match }) => {
	useEffect(
		() => {
			getPost(match.params.id);
		},
		[ getPost, match.params.id ]
	);

	return loading || post === null ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post-header'>
				<Link to='/posts' className='btn'>
					Back To Posts
				</Link>
				<div className='favourites'>
					{/* <Link href='recipes.html' className='btn'>
					Back To Recipes
				</Link> */}
					<a href='/!#'>
						<i className='fas fa-heart fa-2x text-red' /> <p>Add to favourites</p>
					</a>
				</div>
			</div>

			<div className='post-grid my-1'>
				<div className='post-top'>
					<h1 className='text-dark'>{post.nameOfDish}</h1>
					{post.postPhoto.length > 0 && (
						<AwesomeSlider cssModule={AwsSliderStyles}>
							{post.postPhoto.map((photo) => <div key={photo.postPhotoId} data-src={photo.url} />)}
						</AwesomeSlider>
					)}
				</div>

				<div className='post-about bg-light p-2 my-3'>
					<h2 className='text-primary'>Description</h2>
					<p>{post.description}</p>
					<div className='line' />

					<div className='info'>
						<div className='p-1'>
							<i className='fas fa-utensils' />
							{' ' + post.cuisine}
						</div>
						<div className='p-1'>
							<i className='far fa-clock' /> Prep - {post.prepTime}
						</div>
						<div className='p-1'>
							<i className='fas fa-clock' /> Cooking - {post.cookingTime}
						</div>
						<div className='p-1'>
							<i className='fas fa-users' /> Serves {post.feeds}
						</div>
					</div>
				</div>
				<div className='post-ingredients bg-white p-2'>
					<h2 className='text-primary'>Ingredients</h2>
					<div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient{' '}
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
						<div className='p'>
							<i className='fas fa-angle-right' /> Ingredient
						</div>
					</div>
				</div>
				<div className='post-method bg-white p-2'>
					<h2 className='text-primary'>Method</h2>
					<div>
						<div className='p'>1: Cook</div>
						<div className='p'>2: Cook</div>
						<div className='p'>3: Cook</div>
						<div className='p'>
							4: Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook
							Cook Cook Cook Cook Cook Cook Cook Cook Cook {' '}
						</div>
						<div className='p'>1: Cook</div>
						<div className='p'>2: Cook</div>
						<div className='p'>3: Cook</div>
						<div className='p'>1: Cook</div>
						<div className='p'>2: Cook</div>
						<div className='p'>3: Cook</div>
					</div>
				</div>
				<div className='post-footer'>
					<div>
						<h1 className='text-primary'>
							Published by:{' '}
							<Link to='/profile.html'>
								<div className='text-dark'>
									{post.userPhotoUrl ? (
										<img className='icon-b' src={post.userPhotoUrl} />
									) : (
										<i className='far fa-user-circle fa-3x ' />
									)}
									<h2>{post.author}</h2>
								</div>
							</Link>
						</h1>
					</div>
					<button className='comments'>
						<i className='fas fa-comments fa-3x text-primary comments'>{'  ' + post.comments.length}</i>
					</button>
				</div>
			</div>
		</Fragment>
	);
};

Post.propTypes = {
	getPost: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post
});

export default connect(mapStateToProps, { getPost })(Post);
