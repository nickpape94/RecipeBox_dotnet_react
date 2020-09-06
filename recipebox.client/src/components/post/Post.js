import React, { Fragment, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import PostItem from '../posts/PostItem';
import { getPost } from '../../actions/post';

const Post = ({ getPost, post: { post, loading }, match }) => {
	useEffect(
		() => {
			getPost(match.params.id);
		},
		[ getPost ]
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
						<i className='fas fa-heart fa-2x' />
					</a>
				</div>
			</div>

			<div className='post-grid my-1'>
				<div className='post-top'>
					<h1 className='text-dark'>{post.nameOfDish}</h1>
					<img
						className='recipe'
						src='https://www.thespruceeats.com/thmb/5EJU2Kz4m7N3i2tTZe1G_wyzoVc=/1500x844/smart/filters:no_upscale()/classic-southern-fried-chicken-3056867-11_preview-5b106156119fa80036c19a9e.jpeg'
					/>
				</div>

				<div className='post-about bg-light p-2'>
					<h2 className='text-primary'>Description</h2>
					<p>{post.description}</p>
					<div className='line' />

					<div className='info'>
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
							<i className='fas fa-angle-right' /> Ingredient Ingredient Ingredient
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
						<div className='p'>4: Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook Cook </div>
						<div className='p'>1: Cook</div>
						<div className='p'>2: Cook</div>
						<div className='p'>3: Cook</div>
						<div className='p'>1: Cook</div>
						<div className='p'>2: Cook</div>
						<div className='p'>3: Cook</div>
					</div>
				</div>
				<div className='post-footer'>
					<h1 className='text-primary'>Published by:</h1>
					<Link to='/profile.html'>
						<div className='text-dark'>
							{post.userPhotoUrl ? (
								<img className='icon-b' src={post.userPhotoUrl} />
							) : (
								<i class='far fa-user-circle fa-3x ' />
							)}
							<h2>{post.author}</h2>
						</div>
					</Link>
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
