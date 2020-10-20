import React, { Fragment, useEffect, useState } from 'react';
import PropTypes from 'prop-types';
import { Link, Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import Spinner from '../layout/Spinner';
import CommentItem from './CommentItem';
import PostItem from '../posts/PostItem';
import { getPost } from '../../actions/post';
import { addToFavourites, deleteFavourite, getFavourite, getFavourites } from '../../actions/favourite';
import AwesomeSlider from 'react-awesome-slider';
import AwsSliderStyles from 'react-awesome-slider/dist/styles.css';

const Post = ({
	addToFavourites,
	deleteFavourite,
	getFavourite,
	getPost,
	favourite: { favourite, favouritesLoading, error },
	post: { post, loading },
	auth: { user },
	match
}) => {
	const [ loadingPage, setLoadingPage ] = useState(false);
	const [ requestComments, loadComments ] = useState(false);

	useEffect(
		() => {
			getPost(match.params.id, setLoadingPage);
		},
		[ getPost, match.params.id ]
	);

	useEffect(
		() => {
			if (user !== null) {
				getFavourite(user.id, match.params.id);
			}
		},
		[ getFavourite, user, match.params.id ]
	);

	if (loadingPage) {
		return <Spinner />;
	}

	return loading || favouritesLoading || post === null || user === null ? (
		<Spinner />
	) : (
		<Fragment>
			<div className='post-header'>
				<Link to='/posts' className='btn'>
					<i className='fas fa-arrow-circle-left' /> Back To Posts
				</Link>
				<div className='favourites'>
					{/* <Link href='recipes.html' className='btn'>
					Back To Recipes
				</Link> */}

					{(favourite === null || favourite.postId !== post.postId) && (
						<button onClick={() => addToFavourites(user.id, post.postId)}>
							<i className='fas fa-heart fa-2x text-red' /> <p>Add to favourites</p>
						</button>
					)}

					{favourite !== null &&
					favourite.postId === post.postId && (
						<button onClick={() => deleteFavourite(user.id, post.postId)}>
							<i className='fas fa-heart fa-2x text-red' /> <p>Remove from favourites</p>
						</button>
					)}
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
						{post.ingredients.split(',').map((ingredient) => (
							<div className='p' key={ingredient}>
								<i className='fas fa-angle-right' /> {ingredient}
							</div>
						))}
					</div>
				</div>
				<div className='post-method bg-white p-2'>
					<h2 className='text-primary'>Method</h2>
					<div>{[ ...post.method ]}</div>
				</div>
				<div className='post-footer'>
					{/* <div>
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
					</div> */}
					{!requestComments ? (
						<button
							className='comments'
							onClick={() =>
								post.comments.length > 0 ? loadComments(true) : console.log('No comments to load')}
						>
							<i className='fas fa-comments fa-3x text-primary comments'>{'  ' + post.comments.length}</i>
						</button>
					) : (
						<div>
							{post.comments.map((comment) => (
								<CommentItem key={comment.commentId} comment={comment} postId={post.postId} />
							))}
						</div>
					)}
				</div>
			</div>
		</Fragment>
	);
};

Post.propTypes = {
	getPost: PropTypes.func.isRequired,
	getFavourite: PropTypes.func.isRequired,
	post: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired,
	favourite: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	post: state.post,
	auth: state.auth,
	favourite: state.favourite
});

export default connect(mapStateToProps, { getPost, addToFavourites, deleteFavourite, getFavourite })(Post);
